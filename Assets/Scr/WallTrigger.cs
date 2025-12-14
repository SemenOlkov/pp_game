using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public GameObject screamerUI; // ������ �� ���������
    public Animator screamerAnimator; // �������� �������

    private bool isScreaming = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isScreaming)
        {
            StartScream();
        }
    }

    public void StartScream()
    {
        if (isScreaming) return; // ��� ����
        isScreaming = true;

        // �������� UI ��������
        screamerUI.SetActive(true);

        // ��������� ��������
        screamerAnimator.SetTrigger("StartScream");
    }

    // ����� �������� ����� ��� ����������, ������� ���������� �� ������� � ��������
    public void StopScream()
    {
        // ��������� UI
        screamerUI.SetActive(false);
        isScreaming = false;
    }
}